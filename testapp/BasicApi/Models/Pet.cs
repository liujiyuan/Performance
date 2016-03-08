// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasicApi.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public Category Category { get; set; }

        [Required]
        public string Name { get; set; }

        public List<string> Urls { get; set; }

        public List<Tag> Tags { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
