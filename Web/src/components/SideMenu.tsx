import * as React from "react";
import { Drawer, List, ListItem, ListItemIcon, Divider, ListItemSecondaryAction, Icon } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { fetchSeries, Serie } from "../containers/series/reducers";
import { fetchPatients, Patient } from "../containers/patients/reducers";
import { fetchStudies, Study } from "../containers/studies/reducers";
import { fetchInstances, setCurrentInstanceInd } from "../containers/instances/reducers";
import ArrowBackIcon from "@material-ui/icons/ArrowBackOutlined";
import PersonIcon from "@material-ui/icons/PersonOutlined";
import CollectionIcon from "@material-ui/icons/CollectionsOutlined";
import PhotoIcon from "@material-ui/icons/PhotoOutlined";

export interface SideMenuProps {
    readonly patients: Patient[];
    readonly series: Serie[];
    readonly studies: Study[];
    readonly instances: string[];
    readonly fetchSeries: (id: string) => void;
    readonly fetchPatients: () => void;
    readonly fetchStudies: (id: string) => void;
    readonly fetchInstancesIds: (id: string) => void;
    readonly setCurrentInd: (ind: number) => void;
}

interface SideMenuState {
    readonly listType: "patients" | "studies" | "series" | "instances";
    readonly patientId?: string;
    readonly studyId?: string;
}


class SideMenuView extends React.Component<SideMenuProps, SideMenuState> {
    constructor(props) {
        super(props);
        this.state = {
            listType: "patients"
        };
    }
    componentDidMount() {
        this.props.fetchPatients();
    }

    render() {

        return (
            <List><ListItem
                button={this.state.listType !== "patients"}
                onClick={() => {
                    switch (this.state.listType) {
                        case "patients":
                            break;
                        case "studies":
                            this.setState({ listType: "patients" });
                            break;
                        case "series":
                            this.setState({ listType: "studies" });
                            break;
                        case "instances":
                            this.setState({ listType: "series" });
                            break;
                    }
                }}
            >
                <ListItemSecondaryAction>
                    <ListItemIcon>
                        {this.state.listType === "patients" ? null : <ArrowBackIcon />}
                    </ListItemIcon>
                </ListItemSecondaryAction>
                {this.state.listType}
            </ListItem>
                <Divider />
                {this.state.listType === "patients" && <>
                    {this.props.patients
                        .map(p => <ListItem
                            button
                            key={p.id}
                            onClick={() => this.setState({ listType: "studies", patientId: p.id }, () => this.props.fetchStudies(p.id))}
                        >
                            {p.name}
                            <ListItemSecondaryAction>
                                <ListItemIcon>
                                    <PersonIcon />
                                </ListItemIcon>
                            </ListItemSecondaryAction>
                        </ListItem>)}
                </>}
                {this.state.listType === "studies" && this.props.studies
                    .map(s => <ListItem
                        button
                        key={s.id}
                        onClick={() => this.setState({ listType: "series", studyId: s.id }, () => this.props.fetchSeries(s.id))}
                    >
                        {(s.name !== undefined && s.name.length > 0) ? s.name : "No name"}
                        <ListItemSecondaryAction>
                            <ListItemIcon>
                                <CollectionIcon />
                            </ListItemIcon>
                        </ListItemSecondaryAction>
                    </ListItem>)}
                {this.state.listType === "series" && this.props.series.map(s => <ListItem
                    onClick={() => this.setState({ listType: "instances", studyId: s.id }, () => this.props.fetchInstancesIds(s.id))}
                    button key={s.id}>
                    {(s.name !== undefined && s.name.length > 0) ? s.name : "No name"}
                    <ListItemSecondaryAction>
                        <ListItemIcon>
                            <CollectionIcon />
                        </ListItemIcon>
                    </ListItemSecondaryAction>
                </ListItem>)}
                {this.state.listType === "instances" && this.props.instances.map((s, i) => <ListItem
                    key={s}
                    button
                    onClick={() => this.props.setCurrentInd(i)}
                >
                    {i + 1}
                    <ListItemSecondaryAction>
                        <ListItemIcon>
                            <PhotoIcon />
                        </ListItemIcon>
                    </ListItemSecondaryAction>
                </ListItem>)}
            </List >
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            series: state.series,
            patients: state.patients,
            studies: state.studies,
            instances: state.instancesIds
        };
    },
    (dispatch: Dispatch<any>) => ({
        fetchSeries: (id: string) => {
            dispatch(fetchSeries("studies/" + id + "/series"));
        },
        fetchStudies: (id: string) => {
            dispatch(fetchStudies("patients/" + id + "/studies"));
        },
        fetchInstancesIds: (id: string) => {
            dispatch(fetchInstances("series/" + id + "/instances"));
        },
        fetchPatients: () => {
            dispatch(fetchPatients());
        },
        setCurrentInd: (ind: number) => {
            dispatch(setCurrentInstanceInd(ind));
        }
    })
)(SideMenuView);
