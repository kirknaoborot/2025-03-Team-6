using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Shared.Enums;

/// <summary>
/// Статус
/// </summary>
public enum StatusType
{
    [Display(Name = "Новое")]
    New = 0,
    
    [Display(Name = "Распределено")] 
    Distributed = 1,
    
    [Display(Name = "В работе")]
    InWork = 2,
    
    [Display(Name = "Обработано")]
    Closed = 3
}