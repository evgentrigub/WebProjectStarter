﻿using System;

namespace Project.Core.Abstractions.Models
{
    public interface IModelBase
    {
        string Id { get; set; }

        bool IsActive { get; set; }

        DateTime CreatedDate { get; set; }
    }
}