// Copyright (c) 2024 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using AccelByte.ProfanityFilter.Registered.V1;
using AccelByte.PluginArch.ProfanityFilter.Demo.Server.Services;

namespace AccelByte.PluginArch.ProfanityFilter.Demo.Tests
{
    [TestFixture]
    public class ProfanityFilterServiceTests
    {
        private ILogger<ProfanityFilterServiceImpl> _ServiceLogger;

        public ProfanityFilterServiceTests()
        {
            ILoggerFactory loggerFactory = new NullLoggerFactory();
            _ServiceLogger = loggerFactory.CreateLogger<ProfanityFilterServiceImpl>();
        }

        [Test]
        [TestCase("", false)]
        [TestCase("normal", false)]
        [TestCase("you are bad", true)]
        public async Task FilterStringTests(string source, bool isFiltered)
        {
            var service = new ProfanityFilterServiceImpl(_ServiceLogger);

            var response = await service.Validate(new ExtendProfanityValidationRequest()
            {
                Value = source
            }, new UnitTestCallContext());
            
            Assert.IsNotNull(response);
            if (response != null)
            {
                Assert.AreEqual(isFiltered, response.IsProfane);
            }
        }
    }
}