set -e

HOST="$1"
shift
PORT="$1"
shift

TIMEOUT=30
QUIET=0
CMD="$@"

echo "🔄 Aguardando $HOST:$PORT ficar disponível..."

for i in $(seq $TIMEOUT); do
    nc -z "$HOST" "$PORT" >/dev/null 2>&1 && break
    echo "⏳ Tentando conectar em $HOST:$PORT ($i/$TIMEOUT)"
    sleep 1
done

if [ "$i" -eq "$TIMEOUT" ]; then
    echo "❌ Timeout: não conseguiu conectar em $HOST:$PORT após $TIMEOUT segundos."
    exit 1
fi

echo "✅ $HOST:$PORT está acessível, iniciando serviço..."
exec $CMD