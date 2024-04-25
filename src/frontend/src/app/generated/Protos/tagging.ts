// @generated by protobuf-ts 2.9.4
// @generated from protobuf file "Protos/tagging.proto" (package "tagging", syntax proto3)
// tslint:disable
import { Empty } from "../google/protobuf/empty";
import { ServiceType } from "@protobuf-ts/runtime-rpc";
import type { BinaryWriteOptions } from "@protobuf-ts/runtime";
import type { IBinaryWriter } from "@protobuf-ts/runtime";
import { WireType } from "@protobuf-ts/runtime";
import type { BinaryReadOptions } from "@protobuf-ts/runtime";
import type { IBinaryReader } from "@protobuf-ts/runtime";
import { UnknownFieldHandler } from "@protobuf-ts/runtime";
import type { PartialMessage } from "@protobuf-ts/runtime";
import { reflectionMergePartial } from "@protobuf-ts/runtime";
import { MessageType } from "@protobuf-ts/runtime";
/**
 * @generated from protobuf message tagging.Tag
 */
export interface Tag {
    /**
     * @generated from protobuf field: string id = 1;
     */
    id: string;
    /**
     * @generated from protobuf field: string user_id = 2;
     */
    userId: string;
    /**
     * @generated from protobuf field: string name = 3;
     */
    name: string;
}
/**
 * @generated from protobuf message tagging.CreateTagRequest
 */
export interface CreateTagRequest {
    /**
     * @generated from protobuf field: string name = 2;
     */
    name: string;
}
/**
 * @generated from protobuf message tagging.GetTagRequest
 */
export interface GetTagRequest {
    /**
     * @generated from protobuf field: string id = 1;
     */
    id: string;
}
/**
 * @generated from protobuf message tagging.UpdateTagRequest
 */
export interface UpdateTagRequest {
    /**
     * @generated from protobuf field: string id = 1;
     */
    id: string;
    /**
     * @generated from protobuf field: string name = 2;
     */
    name: string;
}
/**
 * @generated from protobuf message tagging.DeleteTagRequest
 */
export interface DeleteTagRequest {
    /**
     * @generated from protobuf field: string id = 1;
     */
    id: string;
}
/**
 * @generated from protobuf message tagging.ListTagsRequest
 */
export interface ListTagsRequest {
}
/**
 * @generated from protobuf message tagging.ListTagsResponse
 */
export interface ListTagsResponse {
    /**
     * @generated from protobuf field: repeated tagging.Tag tags = 1;
     */
    tags: Tag[];
}
// @generated message type with reflection information, may provide speed optimized methods
class Tag$Type extends MessageType<Tag> {
    constructor() {
        super("tagging.Tag", [
            { no: 1, name: "id", kind: "scalar", T: 9 /*ScalarType.STRING*/ },
            { no: 2, name: "user_id", kind: "scalar", T: 9 /*ScalarType.STRING*/ },
            { no: 3, name: "name", kind: "scalar", T: 9 /*ScalarType.STRING*/ }
        ]);
    }
    create(value?: PartialMessage<Tag>): Tag {
        const message = globalThis.Object.create((this.messagePrototype!));
        message.id = "";
        message.userId = "";
        message.name = "";
        if (value !== undefined)
            reflectionMergePartial<Tag>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: Tag): Tag {
        let message = target ?? this.create(), end = reader.pos + length;
        while (reader.pos < end) {
            let [fieldNo, wireType] = reader.tag();
            switch (fieldNo) {
                case /* string id */ 1:
                    message.id = reader.string();
                    break;
                case /* string user_id */ 2:
                    message.userId = reader.string();
                    break;
                case /* string name */ 3:
                    message.name = reader.string();
                    break;
                default:
                    let u = options.readUnknownField;
                    if (u === "throw")
                        throw new globalThis.Error(`Unknown field ${fieldNo} (wire type ${wireType}) for ${this.typeName}`);
                    let d = reader.skip(wireType);
                    if (u !== false)
                        (u === true ? UnknownFieldHandler.onRead : u)(this.typeName, message, fieldNo, wireType, d);
            }
        }
        return message;
    }
    internalBinaryWrite(message: Tag, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        /* string id = 1; */
        if (message.id !== "")
            writer.tag(1, WireType.LengthDelimited).string(message.id);
        /* string user_id = 2; */
        if (message.userId !== "")
            writer.tag(2, WireType.LengthDelimited).string(message.userId);
        /* string name = 3; */
        if (message.name !== "")
            writer.tag(3, WireType.LengthDelimited).string(message.name);
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.Tag
 */
export const Tag = new Tag$Type();
// @generated message type with reflection information, may provide speed optimized methods
class CreateTagRequest$Type extends MessageType<CreateTagRequest> {
    constructor() {
        super("tagging.CreateTagRequest", [
            { no: 2, name: "name", kind: "scalar", T: 9 /*ScalarType.STRING*/ }
        ]);
    }
    create(value?: PartialMessage<CreateTagRequest>): CreateTagRequest {
        const message = globalThis.Object.create((this.messagePrototype!));
        message.name = "";
        if (value !== undefined)
            reflectionMergePartial<CreateTagRequest>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: CreateTagRequest): CreateTagRequest {
        let message = target ?? this.create(), end = reader.pos + length;
        while (reader.pos < end) {
            let [fieldNo, wireType] = reader.tag();
            switch (fieldNo) {
                case /* string name */ 2:
                    message.name = reader.string();
                    break;
                default:
                    let u = options.readUnknownField;
                    if (u === "throw")
                        throw new globalThis.Error(`Unknown field ${fieldNo} (wire type ${wireType}) for ${this.typeName}`);
                    let d = reader.skip(wireType);
                    if (u !== false)
                        (u === true ? UnknownFieldHandler.onRead : u)(this.typeName, message, fieldNo, wireType, d);
            }
        }
        return message;
    }
    internalBinaryWrite(message: CreateTagRequest, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        /* string name = 2; */
        if (message.name !== "")
            writer.tag(2, WireType.LengthDelimited).string(message.name);
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.CreateTagRequest
 */
export const CreateTagRequest = new CreateTagRequest$Type();
// @generated message type with reflection information, may provide speed optimized methods
class GetTagRequest$Type extends MessageType<GetTagRequest> {
    constructor() {
        super("tagging.GetTagRequest", [
            { no: 1, name: "id", kind: "scalar", T: 9 /*ScalarType.STRING*/ }
        ]);
    }
    create(value?: PartialMessage<GetTagRequest>): GetTagRequest {
        const message = globalThis.Object.create((this.messagePrototype!));
        message.id = "";
        if (value !== undefined)
            reflectionMergePartial<GetTagRequest>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: GetTagRequest): GetTagRequest {
        let message = target ?? this.create(), end = reader.pos + length;
        while (reader.pos < end) {
            let [fieldNo, wireType] = reader.tag();
            switch (fieldNo) {
                case /* string id */ 1:
                    message.id = reader.string();
                    break;
                default:
                    let u = options.readUnknownField;
                    if (u === "throw")
                        throw new globalThis.Error(`Unknown field ${fieldNo} (wire type ${wireType}) for ${this.typeName}`);
                    let d = reader.skip(wireType);
                    if (u !== false)
                        (u === true ? UnknownFieldHandler.onRead : u)(this.typeName, message, fieldNo, wireType, d);
            }
        }
        return message;
    }
    internalBinaryWrite(message: GetTagRequest, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        /* string id = 1; */
        if (message.id !== "")
            writer.tag(1, WireType.LengthDelimited).string(message.id);
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.GetTagRequest
 */
export const GetTagRequest = new GetTagRequest$Type();
// @generated message type with reflection information, may provide speed optimized methods
class UpdateTagRequest$Type extends MessageType<UpdateTagRequest> {
    constructor() {
        super("tagging.UpdateTagRequest", [
            { no: 1, name: "id", kind: "scalar", T: 9 /*ScalarType.STRING*/ },
            { no: 2, name: "name", kind: "scalar", T: 9 /*ScalarType.STRING*/ }
        ]);
    }
    create(value?: PartialMessage<UpdateTagRequest>): UpdateTagRequest {
        const message = globalThis.Object.create((this.messagePrototype!));
        message.id = "";
        message.name = "";
        if (value !== undefined)
            reflectionMergePartial<UpdateTagRequest>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: UpdateTagRequest): UpdateTagRequest {
        let message = target ?? this.create(), end = reader.pos + length;
        while (reader.pos < end) {
            let [fieldNo, wireType] = reader.tag();
            switch (fieldNo) {
                case /* string id */ 1:
                    message.id = reader.string();
                    break;
                case /* string name */ 2:
                    message.name = reader.string();
                    break;
                default:
                    let u = options.readUnknownField;
                    if (u === "throw")
                        throw new globalThis.Error(`Unknown field ${fieldNo} (wire type ${wireType}) for ${this.typeName}`);
                    let d = reader.skip(wireType);
                    if (u !== false)
                        (u === true ? UnknownFieldHandler.onRead : u)(this.typeName, message, fieldNo, wireType, d);
            }
        }
        return message;
    }
    internalBinaryWrite(message: UpdateTagRequest, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        /* string id = 1; */
        if (message.id !== "")
            writer.tag(1, WireType.LengthDelimited).string(message.id);
        /* string name = 2; */
        if (message.name !== "")
            writer.tag(2, WireType.LengthDelimited).string(message.name);
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.UpdateTagRequest
 */
export const UpdateTagRequest = new UpdateTagRequest$Type();
// @generated message type with reflection information, may provide speed optimized methods
class DeleteTagRequest$Type extends MessageType<DeleteTagRequest> {
    constructor() {
        super("tagging.DeleteTagRequest", [
            { no: 1, name: "id", kind: "scalar", T: 9 /*ScalarType.STRING*/ }
        ]);
    }
    create(value?: PartialMessage<DeleteTagRequest>): DeleteTagRequest {
        const message = globalThis.Object.create((this.messagePrototype!));
        message.id = "";
        if (value !== undefined)
            reflectionMergePartial<DeleteTagRequest>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: DeleteTagRequest): DeleteTagRequest {
        let message = target ?? this.create(), end = reader.pos + length;
        while (reader.pos < end) {
            let [fieldNo, wireType] = reader.tag();
            switch (fieldNo) {
                case /* string id */ 1:
                    message.id = reader.string();
                    break;
                default:
                    let u = options.readUnknownField;
                    if (u === "throw")
                        throw new globalThis.Error(`Unknown field ${fieldNo} (wire type ${wireType}) for ${this.typeName}`);
                    let d = reader.skip(wireType);
                    if (u !== false)
                        (u === true ? UnknownFieldHandler.onRead : u)(this.typeName, message, fieldNo, wireType, d);
            }
        }
        return message;
    }
    internalBinaryWrite(message: DeleteTagRequest, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        /* string id = 1; */
        if (message.id !== "")
            writer.tag(1, WireType.LengthDelimited).string(message.id);
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.DeleteTagRequest
 */
export const DeleteTagRequest = new DeleteTagRequest$Type();
// @generated message type with reflection information, may provide speed optimized methods
class ListTagsRequest$Type extends MessageType<ListTagsRequest> {
    constructor() {
        super("tagging.ListTagsRequest", []);
    }
    create(value?: PartialMessage<ListTagsRequest>): ListTagsRequest {
        const message = globalThis.Object.create((this.messagePrototype!));
        if (value !== undefined)
            reflectionMergePartial<ListTagsRequest>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: ListTagsRequest): ListTagsRequest {
        return target ?? this.create();
    }
    internalBinaryWrite(message: ListTagsRequest, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.ListTagsRequest
 */
export const ListTagsRequest = new ListTagsRequest$Type();
// @generated message type with reflection information, may provide speed optimized methods
class ListTagsResponse$Type extends MessageType<ListTagsResponse> {
    constructor() {
        super("tagging.ListTagsResponse", [
            { no: 1, name: "tags", kind: "message", repeat: 1 /*RepeatType.PACKED*/, T: () => Tag }
        ]);
    }
    create(value?: PartialMessage<ListTagsResponse>): ListTagsResponse {
        const message = globalThis.Object.create((this.messagePrototype!));
        message.tags = [];
        if (value !== undefined)
            reflectionMergePartial<ListTagsResponse>(this, message, value);
        return message;
    }
    internalBinaryRead(reader: IBinaryReader, length: number, options: BinaryReadOptions, target?: ListTagsResponse): ListTagsResponse {
        let message = target ?? this.create(), end = reader.pos + length;
        while (reader.pos < end) {
            let [fieldNo, wireType] = reader.tag();
            switch (fieldNo) {
                case /* repeated tagging.Tag tags */ 1:
                    message.tags.push(Tag.internalBinaryRead(reader, reader.uint32(), options));
                    break;
                default:
                    let u = options.readUnknownField;
                    if (u === "throw")
                        throw new globalThis.Error(`Unknown field ${fieldNo} (wire type ${wireType}) for ${this.typeName}`);
                    let d = reader.skip(wireType);
                    if (u !== false)
                        (u === true ? UnknownFieldHandler.onRead : u)(this.typeName, message, fieldNo, wireType, d);
            }
        }
        return message;
    }
    internalBinaryWrite(message: ListTagsResponse, writer: IBinaryWriter, options: BinaryWriteOptions): IBinaryWriter {
        /* repeated tagging.Tag tags = 1; */
        for (let i = 0; i < message.tags.length; i++)
            Tag.internalBinaryWrite(message.tags[i], writer.tag(1, WireType.LengthDelimited).fork(), options).join();
        let u = options.writeUnknownFields;
        if (u !== false)
            (u == true ? UnknownFieldHandler.onWrite : u)(this.typeName, message, writer);
        return writer;
    }
}
/**
 * @generated MessageType for protobuf message tagging.ListTagsResponse
 */
export const ListTagsResponse = new ListTagsResponse$Type();
/**
 * @generated ServiceType for protobuf service tagging.TaggingGrpcService
 */
export const TaggingGrpcService = new ServiceType("tagging.TaggingGrpcService", [
    { name: "CreateTag", options: {}, I: CreateTagRequest, O: Tag },
    { name: "GetTag", options: {}, I: GetTagRequest, O: Tag },
    { name: "UpdateTag", options: {}, I: UpdateTagRequest, O: Tag },
    { name: "DeleteTag", options: {}, I: DeleteTagRequest, O: Empty },
    { name: "ListTags", options: {}, I: ListTagsRequest, O: ListTagsResponse }
]);
