#!/usr/bin/env bash
set -euo pipefail

curl https://sfc-repo.snowflakecomputing.com/odbc/linux/3.14.0/... -o /tmp/snowflake.rpm
dnf -y --nogpgcheck localinstall /tmp/snowflake.rpm
