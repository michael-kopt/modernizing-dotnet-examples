#!/usr/bin/env bash
set -euo pipefail

dnf remove unixODBC-utf16 unixODBC-utf16-devel -y
dnf install -y unixODBC-devel
ACCEPT_EULA=Y yum install -y msodbcsql18 mssql-tools18
