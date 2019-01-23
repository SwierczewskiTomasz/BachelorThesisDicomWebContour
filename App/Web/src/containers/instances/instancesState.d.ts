declare interface InstancesState {
    readonly instancesIds: string[];
    readonly currentInstanceId: number;
    readonly pixelSpacing?: string;
    readonly spacingBetweenSlices?: string;
}

declare interface AppState extends InstancesState { }
