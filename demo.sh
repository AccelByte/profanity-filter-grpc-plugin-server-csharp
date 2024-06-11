#!/usr/bin/env bash

# profanity filter demo script

# Requires: bash curl jq

set -e
set -o pipefail

test -n "$AB_CLIENT_ID" || (echo "AB_CLIENT_ID is not set"; exit 1)
test -n "$AB_CLIENT_SECRET" || (echo "AB_CLIENT_SECRET is not set"; exit 1)
test -n "$AB_NAMESPACE" || (echo "AB_NAMESPACE is not set"; exit 1)

set -e
set -o pipefail

test -n "$AB_CLIENT_ID" || (echo "AB_CLIENT_ID is not set"; exit 1)
test -n "$AB_CLIENT_SECRET" || (echo "AB_CLIENT_SECRET is not set"; exit 1)
test -n "$AB_NAMESPACE" || (echo "AB_NAMESPACE is not set"; exit 1)

if [ -z "$GRPC_SERVER_URL" ] && [ -z "$EXTEND_APP_NAME" ]; then
  echo "GRPC_SERVER_URL or EXTEND_APP_NAME is not set"
  exit 1
fi

DEMO_PREFIX='profanity_filter_grpc_demo'

function api_curl()
{
  curl -s -o http_response.out -w '%{http_code}' "$@" > http_code.out
  echo >> http_response.out
  cat http_response.out
}

clean_up()
{
  echo Deleting profanity filter ...
  curl -X DELETE "${AB_BASE_URL}/profanity-filter/v1/admin/namespaces/$AB_NAMESPACE/filters/testfilter" -H "Authorization: Bearer $ACCESS_TOKEN"
}

trap clean_up EXIT

echo Logging in client ...

ACCESS_TOKEN="$(curl -s ${AB_BASE_URL}/iam/v3/oauth/token -H 'Content-Type: application/x-www-form-urlencoded' -u "$AB_CLIENT_ID:$AB_CLIENT_SECRET" -d "grant_type=client_credentials" | jq --raw-output .access_token)"

echo Creating custom profanity filter
curl -X PUT -s "${AB_BASE_URL}/profanity-filter/v1/admin/namespaces/$AB_NAMESPACE/filters/testfilter" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d "{\"description\":\"test\",\"filterConfig\":{\"customServerConfig\":{\"gRPCServerAddress\":\"${GRPC_SERVER_URL}\"},\"type\":\"EXTEND_CUSTOM_SERVER\"}}" >/dev/null

echo Test with bad word ...
api_curl -X POST -s "${AB_BASE_URL}/profanity-filter/v1/admin/namespaces/$AB_NAMESPACE/filters/testfilter/profane" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d "{\"value\":\"fuck\"}"
echo

echo Test with normal word ...
api_curl -X POST -s "${AB_BASE_URL}/profanity-filter/v1/admin/namespaces/$AB_NAMESPACE/filters/testfilter/profane" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d "{\"value\":\"hello\"}"
echo
