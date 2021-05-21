dotnet run -c Release --framework net5.0 \
    --runtimes net5.0 \
    --filter *.Foreaching.* \
    --minIterationCount 3 \
    --maxIterationCount 6 \
    --exporters CSV GitHub GitHub
    
    #--runtimes netcoreapp3.0 mono \