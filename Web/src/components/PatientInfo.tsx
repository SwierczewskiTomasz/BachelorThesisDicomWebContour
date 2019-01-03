import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import {
    Button,
    Dialog,
    DialogActions,
    DialogTitle,
    DialogContent,
    TextField
} from "@material-ui/core";
import { Dispatch } from "redux";
import { anonymizePatient } from "../containers/patients/reducers";

const strong = {
    fontWeight: 700
};

export interface PatientInfoProps {
    readonly instancesIds: string[];
    readonly name?: string;
    readonly birthdate?: string;
    readonly sex?: string;
    readonly institutionName?: string;
    readonly referringPhysicianName?: string;
    readonly studyDate?: string;
    readonly studyDescription?: string;
    readonly anonymizePatient: (name: string, birthdate: string, sex: string) => void;
}

export interface PatientInfoState {
    readonly dialogOpen: boolean;
    readonly name?: string;
    readonly birthdate?: string;
    readonly sex?: string;
}

class PatientInfo extends React.Component<PatientInfoProps, PatientInfoState> {
    constructor(props: PatientInfoProps) {
        super(props);
        this.state = {
            dialogOpen: false
        };
    }

    componentWillReceiveProps(props: PatientInfoProps) {
        this.setState({
            name: props.name,
            birthdate: props.birthdate,
            sex: props.sex
        });
    }

    render() {
        const patientUrl = this.props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[0]
            + "/preview" :
            "";
        return <>
            {this.props.instancesIds.length > 0 && <>
                <p><span style={{ ...strong }}>Name</span>: {this.props.name || "No data"}</p>
                <p><span style={{ ...strong }}>Birthdate</span>: {this.props.birthdate || "No data"}</p>
                <p><span style={{ ...strong }}>Sex</span>: {this.props.sex || "No data"}</p>
                <p><span style={{ ...strong }}>Institution Name</span>: {this.props.institutionName || "No data"}</p>
                <p><span style={{ ...strong }}>Referring Physician Name</span>: {this.props.referringPhysicianName || "No data"}</p>
                <p><span style={{ ...strong }}>Study Date</span>: {this.props.studyDate || "No data"}</p>
                <p><span style={{ ...strong }}>Study Description</span>: {this.props.studyDescription || "No data"}</p>
                <Button
                    variant={"contained"}
                    color={"primary"}
                    onClick={() => this.setState({
                        dialogOpen: true
                    })}
                >
                    Anonymize
                </Button>
                <Dialog
                    open={this.state.dialogOpen}
                >
                    <DialogTitle>
                        Anonymize patient
                    </DialogTitle>
                    <DialogContent>
                        <TextField
                            margin="dense"
                            label={"Patient Name"}
                            type="text"
                            value={this.state.name}
                            onChange={e => this.setState({ name: e.target.value })}
                            fullWidth
                            autoFocus
                        />
                        <TextField
                            margin="dense"
                            label={"Birthdate"}
                            type="text"
                            value={this.state.birthdate}
                            onChange={e => this.setState({ birthdate: e.target.value })}
                            fullWidth
                            autoFocus
                        />
                        <TextField
                            margin="dense"
                            label={"Patient sex"}
                            type="text"
                            value={this.state.sex}
                            onChange={e => this.setState({ sex: e.target.value })}
                            fullWidth
                            autoFocus
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button
                            variant={"contained"}
                            color={"primary"}
                            onClick={() => {
                                this.props.anonymizePatient(this.state.name, this.state.birthdate, this.state.sex);
                                this.setState({
                                    dialogOpen: false
                                });
                            }}
                        >
                            Anonymize
                        </Button>
                        <Button
                            variant={"contained"}
                            color={"secondary"}
                            onClick={() => this.setState({ dialogOpen: false })}
                        >
                            Cancel
                </Button>
                    </DialogActions>
                </Dialog>
            </>
            }
        </>;
    }
}
export default connect(
    (state: AppState) => {
        return {
            instancesIds: state.instancesIds,
            name: state.name,
            birthdate: state.birthdate,
            sex: state.sex,
            institutionName: state.institutionName,
            referringPhysicianName: state.referringPhysicianName,
            studyDate: state.studyDate,
            studyDescription: state.studyDescription
        };
    },
    (dispatch: Dispatch<any>) => ({
        anonymizePatient: (name: string, birthdate: string, sex: string) => {
            dispatch(anonymizePatient(name, birthdate, sex));
        }
    })
)(PatientInfo);
