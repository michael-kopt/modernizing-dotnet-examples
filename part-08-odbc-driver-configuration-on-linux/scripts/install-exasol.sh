#!/usr/bin/env bash
set -euo pipefail

curl https://x-up.s3.amazonaws.com/7.x/<version>/... -o /tmp/exasol.tar.gz
tar -xzf /tmp/exasol.tar.gz -C /tmp
mv /tmp/EXASolution_ODBC-* /usr/lib64/
