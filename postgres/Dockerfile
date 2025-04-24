FROM postgres:17.4

RUN echo '#!/bin/bash\n\
set -e\n\
\n\
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL\n\
    CREATE DATABASE "AuthDb";\n\
    CREATE DATABASE "ProductDb";\n\
    CREATE DATABASE "BasketDb";\n\
    CREATE DATABASE "OrderDb";\n\
    CREATE DATABASE "PaymentDb";\n\
    CREATE DATABASE "ReviewDb";\n\
    CREATE DATABASE "NotificationDb";\n\
    CREATE DATABASE "InventoryDb";\n\
EOSQL' > /docker-entrypoint-initdb.d/init-db.sh \
    && chmod +x /docker-entrypoint-initdb.d/init-db.sh 