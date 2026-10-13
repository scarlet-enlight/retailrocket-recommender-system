namespace RetailRocket.Application.DTOs.Request.Shop.Users;

public record PasswordChangeRequestDto()
{
    public string OldPassword { get; set;  }
    public string NewPassword { get; set;  }
    public string RepeatPassword { get; set;  }
}