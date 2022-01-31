using System;
namespace SocialFood.API.Models;

public record StreamFileContent(Stream Content, string ContentType, string FileName, int Length);
