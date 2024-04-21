#!/bin/bash

cd "$(dirname "$0")/.."

PROTO_DIR="../backend/CodeSprint.Common/Protos"
OUT_DIR="./src/app/generated"

mkdir -p ${OUT_DIR}

for proto_file in ${PROTO_DIR}/*.proto; do
    protoc \
    --plugin=protoc-gen-ts=node_modules/.bin/protoc-gen-ts.cmd \
    --ts_out=${OUT_DIR} \
    -I ${PROTO_DIR} \
    ${proto_file}
done

echo "TypeScript service definitions generated."
