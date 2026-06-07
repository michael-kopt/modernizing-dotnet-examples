#!/usr/bin/env bash
set -euo pipefail

curl https://s3.amazonaws.com/redshift-downloads/drivers/odbc/2.1.12/... -o /tmp/redshift.rpm
dnf -y --nogpgcheck localinstall /tmp/redshift.rpm
