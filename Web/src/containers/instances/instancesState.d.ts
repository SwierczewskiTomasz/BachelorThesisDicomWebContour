declare interface InstancesState {
    readonly instancesIds: string[];
    readonly currentInstanceId: number;
    readonly pixelSpacing?: string;
    readonly spacingBetweenSlices?: string;
    readonly seriesName: string | undefined;
    readonly studyName: string | undefined;
    readonly patientName: string | undefined;
}

declare interface AppState extends InstancesState { }
