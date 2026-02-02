// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Common.Utilities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// An utility class for maximum length constants.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class MaxLenght
    {
        /// <summary>
        /// Gets the maximum lenght supported by an entity decorated with StringLength attribute.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">The property to get the max lenght for.</param>
        /// <returns>The maximum supported length.</returns>
        public static int Of<T>(Expression<Func<T, string>> property)
        {
            if (property.Body is not MemberExpression member)
            {
                throw new ArgumentException("Must be a property expression", nameof(property));
            }

            var prop = (PropertyInfo)member.Member;

            var stringLengthAttribute = prop.GetCustomAttribute<StringLengthAttribute>();
            if (stringLengthAttribute is not null)
            {
                return stringLengthAttribute.MaximumLength;
            }

            var maxLen = prop.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLen is not null)
            {
                return maxLen.Length;
            }

            throw new InvalidOperationException($"No StringLength/MaxLength attribute found on {typeof(T).Name}.{prop.Name}");
        }
    }
}
