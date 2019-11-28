dotnet run -c Release --framework netcoreapp3.0 \
    --runtimes netcoreapp3.0 \
    --filter *.ZeroLinq.* \
    --minIterationCount 3 \
    --maxIterationCount 6 \
    --exporters CSV GitHub GitHub
    
    #--runtimes netcoreapp3.0 mono \