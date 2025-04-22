﻿    using FuarYonetimSistemi.Domain.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace FuarYonetimSistemi.Domain.Entities
    {
        public class User
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string FullName { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty;

            public UserRole Role { get; set; } = UserRole.SalesPerson;

            public bool IsActive { get; set; } = true;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public string? PasswordResetToken { get; set; }
            public DateTime? PasswordResetTokenExpires { get; set; }
    }
    }
