﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using Crichton.Representors.Serializers;

namespace Crichton.Client
{
    public class CrichtonClient : ITransitionRequestor
    {
        public HttpClient HttpClient { get; private set; }
        public ISerializer Serializer { get; private set; }
        public Uri BaseUri { get; private set; }

        public CrichtonClient(HttpClient httpClient, Uri baseUri, ISerializer serializer)
        {
            HttpClient = httpClient;
            Serializer = serializer;
            BaseUri = baseUri;
        }

        public IHypermediaQuery CreateQuery()
        {
            return new HypermediaQuery();
        }

        public IHypermediaQuery CreateQuery(CrichtonRepresentor representor)
        {
            var query = new HypermediaQuery();
            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }

        public Task<CrichtonRepresentor> ExecuteQueryAsync(IHypermediaQuery query)
        {
            return query.ExecuteAsync(this);
        }

        public async Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(transition.Uri, UriKind.RelativeOrAbsolute));

            var result = await HttpClient.SendAsync(requestMessage);

            var resultContentString = await result.Content.ReadAsStringAsync();

            var builder = Serializer.DeserializeToNewBuilder(resultContentString, () => new RepresentorBuilder());

            return builder.ToRepresentor();
        }

        public Task<CrichtonRepresentor> PostTransitionDataAsync(CrichtonTransition transition, object toSerializeToJson)
        {
            throw new NotImplementedException();
        }
    }
}
