"""
Automated tests for the ML recommendation service configuration,
FastAPI endpoints, and Docker deployment settings.
"""

from pathlib import Path
import sys
import unittest
from unittest.mock import patch
import uuid

from fastapi import HTTPException

# Add ml-service root to sys.path
ML_SERVICE_ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ML_SERVICE_ROOT))

import src.main as main_module


class TestMLServiceAndDockerConfig(unittest.TestCase):
    """Verify FastAPI endpoints, API key protection, and Docker configuration."""

    @classmethod
    def setUpClass(cls) -> None:
        main_module.refresh_rules_cache()

    def test_health_endpoint(self) -> None:
        response = main_module.health_check()
        self.assertEqual(response.status, "ok")
        self.assertIn(response.source, ("postgresql", "local_csv"))
        self.assertGreater(response.rules_count, 0)

    def test_get_top_rules_returns_valid_uuid_rule_ids(self) -> None:
        rules = main_module.get_top_rules(limit=5)
        self.assertEqual(len(rules), 5)
        for rule in rules:
            parsed_uuid = uuid.UUID(str(rule["rule_id"]))
            self.assertEqual(str(parsed_uuid), rule["rule_id"])
            self.assertGreater(rule["lift"], 1.0)

    def test_required_item_and_result_item_recommendations(self) -> None:
        sample_rule = main_module.get_top_rules(limit=1)[0]
        if_id = int(sample_rule["if_item_id"])
        then_id = int(sample_rule["then_item_id"])

        req_res = main_module.get_recommendations_for_item(item_id=if_id, top_k=5)
        self.assertEqual(req_res.item_id, if_id)
        self.assertGreaterEqual(req_res.count, 1)
        self.assertTrue(any(r.then_item_id == then_id for r in req_res.recommendations))

        res_target = main_module.get_recommendations_by_result_item(item_id=then_id, top_k=5)
        self.assertEqual(res_target.item_id, then_id)
        self.assertGreaterEqual(res_target.count, 1)
        self.assertTrue(any(r.if_item_id == if_id for r in res_target.recommendations))

    def test_train_endpoint_fail_closed_without_api_key(self) -> None:
        with patch.object(main_module, "ML_API_KEY", ""):
            with self.assertRaises(HTTPException) as ctx:
                main_module.verify_internal_api_key(x_api_key="any-key")
            self.assertEqual(ctx.exception.status_code, 403)

    def test_train_endpoint_rejects_invalid_api_key(self) -> None:
        with patch.object(main_module, "ML_API_KEY", "secret-test-key"):
            with self.assertRaises(HTTPException) as ctx:
                main_module.verify_internal_api_key(x_api_key="wrong-key")
            self.assertEqual(ctx.exception.status_code, 401)

            # Matching key succeeds without raising
            main_module.verify_internal_api_key(x_api_key="secret-test-key")

    def test_dockerfile_and_compose_security_constraints(self) -> None:
        dockerfile_path = ML_SERVICE_ROOT / "Dockerfile"
        compose_path = ML_SERVICE_ROOT.parent / "database" / "docker-compose.yml"

        self.assertTrue(dockerfile_path.exists(), "ml-service/Dockerfile must exist")
        dockerfile_text = dockerfile_path.read_text(encoding="utf-8")
        self.assertIn("ML_HOST=0.0.0.0", dockerfile_text)
        self.assertIn("EXPOSE 8000", dockerfile_text)

        compose_text = compose_path.read_text(encoding="utf-8")
        self.assertIn("ml-service:", compose_text)
        self.assertIn("ML_HOST: 0.0.0.0", compose_text)
        self.assertIn('expose:\n      - "8000"', compose_text)

        # Ensure ml-service does not publish port 8000 to the host
        ml_section = compose_text.split("ml-service:", maxsplit=1)[1]
        self.assertNotIn("ports:", ml_section)


if __name__ == "__main__":
    unittest.main()
