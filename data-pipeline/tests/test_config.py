import pytest

from data_pipeline.config import DBConfig


def test_missing_password_raises():
    cfg = DBConfig(host="h", port=1, name="n", user="u", password="")

    with pytest.raises(RuntimeError, match="PASSWORD"):
        cfg.connection_string


def test_special_characters_are_escaped():
    cfg = DBConfig(host="h", port=1, name="n", user="u", password="p@ss word")

    assert "p%40ss%20word" in cfg.connection_string
