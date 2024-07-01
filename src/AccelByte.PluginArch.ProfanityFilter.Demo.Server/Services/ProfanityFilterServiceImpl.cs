// Copyright (c) 2024 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Grpc.Core;
using AccelByte.ProfanityFilter.Registered.V1;
using ProfanityFilterLib = ProfanityFilter.ProfanityFilter;

namespace AccelByte.PluginArch.ProfanityFilter.Demo.Server.Services
{
    public class ProfanityFilterServiceImpl : ProfanityFilterService.ProfanityFilterServiceBase
    {
        private readonly ILogger<ProfanityFilterServiceImpl> _Logger;

        private ProfanityFilterLib _Filter;

        public ProfanityFilterServiceImpl(ILogger<ProfanityFilterServiceImpl> logger)
        {
            _Logger = logger;
            _Filter = new ProfanityFilterLib(new []{ "bad", "ibad", "yourbad" });
        }

        public override Task<ExtendProfanityValidationResponse> Validate(ExtendProfanityValidationRequest request, ServerCallContext context)
        {
            var profaneWords = _Filter.DetectAllProfanities(request.Value);
            bool isProfane = ((profaneWords != null) && (profaneWords.Count > 0));

            return Task.FromResult(new ExtendProfanityValidationResponse()
            {
                IsProfane = isProfane,
                Message = (isProfane ? "this contains banned words" : "")
            });
        }
    }
}
