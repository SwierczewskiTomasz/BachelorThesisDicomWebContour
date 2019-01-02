import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import { Button, Typography } from "@material-ui/core";

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
}

class PatientInfo extends React.Component<PatientInfoProps> {
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
                >
                    Anonymize
                </Button>
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
    }
)(PatientInfo);
