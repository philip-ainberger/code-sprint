import { RpcInterceptor, NextUnaryFn, MethodInfo, RpcOptions } from "@protobuf-ts/runtime-rpc";

export function GrpcAuthInterceptor(token: string): RpcInterceptor {
    return {
        interceptUnary(next: NextUnaryFn, method: MethodInfo<any, any>, input: object, options: RpcOptions) {
            const modifiedOptions: RpcOptions = {
                ...options,
                meta: {
                    ...options.meta,
                    'Authorization': `Bearer ${token}`
                }
            };

            return next(method, input, modifiedOptions);
        }
    };
}