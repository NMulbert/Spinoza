FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# Install the simulator tool
RUN dotnet tool install -g Microsoft.Azure.SignalR.Emulator --version 1.0.0-preview1-10809
ENV PATH="$PATH:/root/.dotnet/tools"

# create a current working directory
RUN mkdir /workspace
WORKDIR /workspace

COPY /LocalEnvironmentSupport/settings.json /workspace/
# init default settings.json
RUN asrs-emulator upstream init

# configurations
EXPOSE ${ASRS_EMULATOR_PORT}

# start the emulator
ENTRYPOINT asrs-emulator start --port $ASRS_EMULATOR_PORT