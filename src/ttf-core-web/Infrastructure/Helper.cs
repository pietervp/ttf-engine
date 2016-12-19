﻿using System;

namespace Ttf.Server.Core.Web.Infrastructure
{
    internal static class Helpers
    {
        /// <summary>
        /// Attempts to detect if the content type is JSON.
        /// Supports:
        ///   application/json
        ///   text/json
        ///   application/vnd[something]+json
        /// Matches are case insentitive to try and be as "accepting" as possible.
        /// </summary>
        /// <param name="contentType">Request content type</param>
        /// <returns>True if content type is JSON, false otherwise</returns>
        public static bool IsJsonType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
                   contentMimeType.Equals("text/json", StringComparison.OrdinalIgnoreCase) ||
                   (contentMimeType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase) &&
                    contentMimeType.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
        }
    }
}