using System.Net.Mail;

namespace Order.Domain.Shared.Validators;

/// <summary>
/// 邮箱格式校验器（领域通用，符合RFC 5322标准）
/// </summary>
internal static class EmailValidator
{
    // 超时时间：防止MailAddress构造函数极端场景下卡顿
    private const int ValidationTimeoutMs = 100;

    /// <summary>
    /// 校验邮箱格式是否合法（适配“必填”场景：空邮箱返回false）
    /// </summary>
    /// <param name="email">待校验的邮箱字符串</param>
    /// <returns>合法则返回<see cref="bool">true</see>，不合法则返回<see cref="bool">false</see></returns>
    internal static bool IsValid(string? email)
    {
        // 场景1：必填邮箱 → 空邮箱直接非法
        if (string.IsNullOrWhiteSpace(email)) return false;

        return ValidateEmailFormat(email);
    }

    /// <summary>
    /// 校验邮箱格式是否合法（适配“可选”场景：空邮箱返回true）
    /// </summary>
    /// <param name="email">待校验的邮箱字符串</param>
    /// <returns>空邮箱/合法邮箱返回<see cref="bool">true</see>，格式错误返回<see cref="bool">false</see></returns>
    internal static bool IsValidOptional(string? email)
    {
        // 场景2：可选邮箱 → 空邮箱视为合法
        if (string.IsNullOrWhiteSpace(email)) return true;

        return ValidateEmailFormat(email);
    }

    /// <summary>
    /// 校验邮箱格式（必填场景），非法则抛出领域异常
    /// </summary>
    /// <param name="email">待校验的邮箱字符串</param>
    /// <exception cref="DomainException">空邮箱/格式错误时抛出</exception>
    internal static void EnsureValid(string? email)
    {
        if (!IsValid(email))
        {
            var errorMsg = string.IsNullOrWhiteSpace(email)
                ? "邮箱不能为空"
                : $"邮箱格式非法: {email}（需符合RFC 5322标准，如 user@example.com）";
            throw new DomainException(errorMsg);
        }
    }

    /// <summary>
    /// 校验邮箱格式（可选场景），格式错误则抛出领域异常（空邮箱不抛异常）
    /// </summary>
    /// <param name="email">待校验的邮箱字符串</param>
    /// <exception cref="DomainException">格式错误时抛出</exception>
    internal static void EnsureValidOptional(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        if (!ValidateEmailFormat(email))
        {
            throw new DomainException($"邮箱格式非法: {email}（需符合RFC 5322标准，如 user@example.com）");
        }
    }

    /// <summary>
    /// 核心格式校验逻辑（复用）
    /// </summary>
    /// <param name="email">非空的邮箱字符串</param>
    /// <returns>格式是否合法</returns>
    private static bool ValidateEmailFormat(string email)
    {
        try
        {
            // 限制字符串长度（RFC标准：邮箱最大长度254字符）
            if (email.Length > 254) return false;

            // 带超时的格式校验（防止极端场景卡顿）
            var isValid = System.Threading.Tasks.Task.Run(() =>
            {
                _ = new MailAddress(email);
                return true;
            }).Wait(ValidationTimeoutMs) && System.Threading.Tasks.Task.Run(() => new MailAddress(email)).Result != null;

            return isValid;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (Exception)
        {
            // 捕获超时/其他异常，判定为非法
            return false;
        }
    }
}