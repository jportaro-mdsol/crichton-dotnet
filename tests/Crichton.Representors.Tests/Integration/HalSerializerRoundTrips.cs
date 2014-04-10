﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Representors.Tests.Integration
{
    public class HalSerializerRoundTrips : TestWithFixture
    {
        private HalSerializer serializer;
        private RepresentorBuilder builder;

        [SetUp]
        public void Init()
        {
            serializer = new HalSerializer();
            builder = new RepresentorBuilder();
            Fixture = GetFixture();
        }

        private const string SelfLinkOnly = @"{
            '_links': {
                'self': { 'href': '/example_resource' }
            }
        }";

        [Test]
        public void SelfLinkOnly_RoundTrip()
        {
            TestRoundTripJson(SelfLinkOnly);
        }

        private const string MultipleLinksSameRelation = @"
        {
            '_links': {
              'items': [{
                  'href': '/first_item'
              },{
                  'href': '/second_item'
              }]
            }
        }";

        [Test]
        public void MultipleLinksSameRelation_RoundTrip()
        {
            TestRoundTripJson(MultipleLinksSameRelation);
        }

        private const string SimpleLinksAndAttributes = @"{
        '_links': {
            'self': { 'href': '/orders' },
            'next': { 'href': '/orders?page=2' },
            'ea:find': {
                'href': '/orders{?id}'
            },
            'ea:admin': [{
                'href': '/admins/2',
                'title': 'Fred'
            }, {
                'href': '/admins/5',
                'title': 'Kate'
            }]
        },
        'currentlyProcessing': 14,
        'shippedToday': 20,
        }";

        [Test]
        public void SimpleLinksAndAttributes_RoundTrip()
        {
            TestRoundTripJson(SimpleLinksAndAttributes);
        }

        public void TestRoundTripJson(string json)
        {
            var expected = JObject.Parse(json).ToString();

            serializer.DeserializeToBuilder(expected, builder);

            var result = serializer.Serialize(builder.ToRepresentor());

            Assert.AreEqual(expected, result);
        }

    }
}
