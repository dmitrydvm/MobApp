namespace MobApps.Models
{
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Объект ошибки
        /// </summary>
        /// <param name="code">Стандартный код ошибки</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="description">Подробное описание ошибки или описание Inner Exception</param>
        public Error(string code, string message, string description = "")
        {
            Code = code;

            Message = message;

            Description = description ?? string.Empty;
        }
    }
}
