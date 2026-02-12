using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

public class CreateCategoryDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = "";
}