#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE DATABASE "AuthDb";
    CREATE DATABASE "ProductDb";
    CREATE DATABASE "BasketDb";
    CREATE DATABASE "OrderDb";
    CREATE DATABASE "PaymentDb";
    CREATE DATABASE "ReviewDb";
    CREATE DATABASE "NotificationDb";
    CREATE DATABASE "InventoryDb";
EOSQL 