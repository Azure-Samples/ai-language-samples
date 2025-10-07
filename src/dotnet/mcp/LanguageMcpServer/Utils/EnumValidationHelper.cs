// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Azure.AI.Language.MCP.Server.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Azure.AI.Language.MCP.Server.Utils
{
    /// <summary>
    /// Provides helper methods for validating and converting string values to enum types.
    /// </summary>
    internal static class EnumValidationHelper
    {
        /// <summary>
        /// Converts the specified string to the corresponding enum value of type <typeparamref name="TEnum"/>.
        /// Throws an <see cref="ArgumentException"/> if the value is not valid for the enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to convert to.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <returns>The corresponding enum value.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid enum name.</exception>
        public static TEnum ToEnumValue<TEnum>(this string value)
            where TEnum : struct, Enum
        {
            if (!Enum.TryParse(value, true, out TEnum enumValue))
            {
                var allowedValues = string.Join(", ", Enum.GetNames(typeof(TEnum)));
                throw new ArgumentException($"Invalid value '{value}'. Allowed values are: {allowedValues}.", nameof(value));
            }

            return enumValue;
        }

        /// <summary>
        /// Converts a list of string values to a list of corresponding enum values of type <typeparamref name="TEnum"/>.
        /// Throws an <see cref="ArgumentException"/> if any value is not valid for the enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to convert to.</typeparam>
        /// <param name="values">The list of string values to convert.</param>
        /// <returns>A list of corresponding enum values.</returns>
        /// <exception cref="ArgumentException">Thrown when any value is not a valid enum name.</exception>
        public static IList<TEnum> ToEnumValues<TEnum>(this IList<string> values)
            where TEnum : struct, Enum
        {
            var enumValues = new List<TEnum>();

            foreach (var value in values)
            {
                enumValues.Add(value.ToEnumValue<TEnum>());
            }

            return enumValues;
        }
    }
}