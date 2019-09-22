using System;
using API.Core.Abstractions.Models;
using Project.Core.Abstractions.Models;

namespace API.Models.Base
{
    public abstract class ModelBase : IModelBase
    {
        protected ModelBase()
        {
            Id = Guid.NewGuid().ToString();
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDate { get; set; }
    }
}