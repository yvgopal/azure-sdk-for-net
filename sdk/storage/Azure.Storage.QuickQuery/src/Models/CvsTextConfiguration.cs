// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Storage.QuickQuery.Models
{
    /// <summary>
    /// CSV text configuration.
    /// </summary>
    public class CvsTextConfiguration : BlobQueryTextConfiguration
    {
        /// <summary>
        /// Column separator.
        /// </summary>
        public string ColumnSeparator { get; set; }

        /// <summary>
        /// Field quote.
        /// </summary>
        public string FieldQuote { get; set; }

        /// <summary>
        /// Escape character.
        /// </summary>
        public char EscapeCharacter { get; set; }
    }
}
