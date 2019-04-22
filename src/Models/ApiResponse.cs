using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobApps.Models
{
    /// <summary>
    /// Универсальный объект ответа API, инкапсулирующий все нужные свойства.
    /// </summary>
    /// <typeparam name="T">Тип данных в свойстве Data</typeparam>
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public ReadOnlyCollection<Error> Errors { get; }

        /// <summary>
        /// Признак успешности ответа. True если список ошибок пустой. Генерируется автоматически.
        /// </summary>
        public bool IsSuccess => Errors == null;

        /// <summary>
        /// ID ответа. Для каждого ответа геренируется уникальный ID. Используется для идентификации ответа.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Сквозной ID запроса для отслеживания ошибки в Trace-логах.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Конструктор по-умолчанию. Используется по мере надобности.
        /// </summary>
        public ApiResponse()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Полный конструктор. Используется в блоке Catch.
        /// </summary>
        /// <param name="code">Стандартный код ошибки</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="description">Подробное описание ошибки или описание Inner Exception</param>
        public ApiResponse(string code, string message, string description = "")
            : this()
        {
            Errors = new List<Error>
            {
                new Error(code, message, description ?? string.Empty)
            }
            .AsReadOnly();
        }

        /// <summary>
        /// Конструктор для передачи данных. Используется, когда надо передать только данные.
        /// </summary>
        /// <param name="data">Объект данных для передачи</param>
        public ApiResponse(T data)
            : this()
        {
            Data = data;
        }

        /// <summary>
        /// Конструктор для ошибкок. Используется, когда надо передать заготовленный список ошибок.
        /// </summary>
        /// <param name="errors">Список ошибок</param>
        public ApiResponse(ICollection<Error> errors)
            : this()
        {
            Errors = errors.ToList().AsReadOnly();
        }

        /// <summary>
        /// Возвращает все содержимое объекта в сериализованном виде.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
