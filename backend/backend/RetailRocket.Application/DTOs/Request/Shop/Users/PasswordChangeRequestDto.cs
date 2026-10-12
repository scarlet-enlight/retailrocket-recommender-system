namespace RetailRocket.Application.DTOs.Request.Shop.Users;

public record PasswordChangeRequestDto()
{
    public string OldPassword { get; }
    public string NewPassword { get; }
    public string RepeatPassword { get; }
}