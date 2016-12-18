using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.IO;
using Nancy.Responses.Negotiation;
using NetJSON;

namespace Ttf.Server.Core.Web.Infrastructure
{
    public class NetJsonSerializer : ISerializer
    {
        #region Implementation of ISerializer

        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to serialize</param>
        /// <returns>
        /// True if supported, false otherwise
        /// </returns>
        public bool CanSerialize(MediaRange mediaRange)
        {
            return Helpers.IsJsonType($"{mediaRange.Type}/{mediaRange.Subtype}");
        }

        /// <summary>
        /// Serializes the given model instance with the given contentType
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="mediaRange">Content type to serialize into</param>
        /// <param name="model">Model instance to serialize.</param>
        /// <param name="outputStream">Output stream to serialize to.</param>
        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            NetJSONSettings.CurrentSettings.DateFormat = NetJSONDateFormat.ISO;
            NetJSONSettings.CurrentSettings.Format = NetJSONFormat.Prettify;
            NetJSONSettings.CurrentSettings.QuoteType = NetJSONQuote.Default;
            NetJSON.NetJSON.IncludeFields = false;

            using (var output = new StreamWriter(new UnclosableStreamWrapper(outputStream)))
            {
                NetJSON.NetJSON.Serialize(model, output);
            }
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of extensions if any are available, otherwise an empty enumerable.
        /// </value>
        public IEnumerable<string> Extensions
        {
            get { yield return "json"; }
        }

        #endregion
    }
}
