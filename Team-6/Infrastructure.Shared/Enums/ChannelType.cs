using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Shared.Enums
{
    /// <summary>
    /// Тип текстовых каналов
    /// </summary>
    public enum ChannelType
    {
        [Display(Name = "Телеграмм")]
        Telegram = 0,

        [Display(Name = "VK")]
        Vk = 1,

        [Display(Name = "EMAIL")]
        Email = 2
    }
}
