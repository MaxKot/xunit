using System;
using Xunit.Internal;

namespace Xunit.Sdk;

/// <summary>
/// Used to decorate concrete serializable classes that derive from <see cref="MessageSinkMessage"/> to
/// indicate what the serialized type ID should be. The type IDs must be unique, and only assigned to
/// concrete types that will be serialized and deserialized.
/// </summary>
/// <remarks>
/// These types are made public for third parties only for the purpose of serializing and
/// deserializing messages that are sent across the process boundary (that is, types which
/// derived directly or indirectly from <see cref="MessageSinkMessage"/>). Any other usage
/// is not supported.
/// </remarks>
/// <param name="id">The JSON type ID</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class JsonTypeIDAttribute(string id) : Attribute
{
	/// <summary/>
	public string ID { get; } = Guard.ArgumentNotNull(id);
}
