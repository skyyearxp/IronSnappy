# IronSnappy

This is a native .NET port of [Google Snappy](https://github.com/google/snappy) compression/decompression library. The only implementation that is stable, fast, up to date with latest Snappy improvements, and most importantly *does not depend on native Snappy binaries*. 

It is originally ported from the [Golang implementation](https://github.com/golang/snappy/) because Go is much easier to understand and work with comparing to C++.

The library passes *golden tests* from the original implementation i.e. compares that compression/decompression is fully compatible with the original implementation.

Internally, it is using array pooling and spans for efficient memory allocation and low GC pressure.

## Using

Reference the following 



## Contributing

Contributions are more than welcome, just raise an issue and fire a PR. The code might have a few ugly bits due to the fact it was ported as is from Golang, you are welcome to make it prettier and/or faster.