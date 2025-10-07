// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Server.Tests
{
    public class TestResponse<T> : Response<T>
    {
        private readonly T content;

        public TestResponse(T content)
        {
            this.content = content;
        }

        public override T Value => content;

        public override Azure.Response GetRawResponse() => null;

    }
}
