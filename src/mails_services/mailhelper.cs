using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class MailAccount
{
    public string Address { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class MailAccountStorage
{
    private static readonly JsonSerializerOptions _opts = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task AddAccountToListAsync(string filePath, MailAccount account)
    {
        var list = await LoadAccountListAsync(filePath).ConfigureAwait(false) ?? new List<MailAccount>();
        list.Add(account);
        string json = JsonSerializer.Serialize(list, _opts);
        await File.WriteAllTextAsync(filePath, json).ConfigureAwait(false);
    }

    public static async Task<List<MailAccount>> LoadAccountListAsync(string filePath)
    {
        if (!File.Exists(filePath)) return null;
        string json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        try
        {
            return JsonSerializer.Deserialize<List<MailAccount>>(json, _opts);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<bool> UpdateTokenAsync(string filePath, string address, string newToken)
    {
        var list = await LoadAccountListAsync(filePath).ConfigureAwait(false);
        if (list == null) return false;
        var acc = list.Find(a => string.Equals(a.Address, address, StringComparison.OrdinalIgnoreCase));
        if (acc == null) return false;
        acc.Token = newToken;
        string json = JsonSerializer.Serialize(list, _opts);
        await File.WriteAllTextAsync(filePath, json).ConfigureAwait(false);
        return true;
    }
}