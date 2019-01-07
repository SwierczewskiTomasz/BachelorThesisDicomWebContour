declare interface InstancesState {
    readonly instancesIds: string[];
    readonly currentInstanceId: number;
}

declare interface AppState extends InstancesState { }
