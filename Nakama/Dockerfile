FROM node:alpine AS node-builder

WORKDIR /backend

COPY package*.json .
RUN npm install

COPY tsconfig.json .
COPY src/*.ts src/
RUN npx tsc

FROM registry.heroiclabs.com/heroiclabs/nakama:3.17.1

COPY --from=node-builder /backend/build/*.js /nakama/data/modules/build/
COPY local.yml .
COPY local-test.yml .