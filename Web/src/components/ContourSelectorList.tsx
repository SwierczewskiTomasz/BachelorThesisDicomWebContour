import * as React from "react";
import { Drawer, List, ListItem, ListItemIcon, Divider, ListItemSecondaryAction } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import AddIcon from "@material-ui/icons/AddOutlined";
import RemoveIcon from "@material-ui/icons/RemoveOutlined";
import { Contour, addSelectedContour, removeSelectedContour, } from "../containers/contours/reducers";

export interface ContourListProps {
    readonly contours: Contour[];
    readonly selectedContourGuids: string[];
    readonly addSelectedContour: (guid: string) => void;
    readonly removeSelectedContour: (guid: string) => void;
}

class ContourListView extends React.Component<ContourListProps> {
    render() {
        return (
            <>
                <List>
                    <ListItem>
                        {"Available Contours"}
                    </ListItem>
                    <Divider />
                    {this.props.contours
                        .map(c => <ListItem
                            button
                            key={c.guid}
                            onClick={() => this.props.selectedContourGuids.find(sc => sc === c.guid) !== undefined ?
                                this.props.removeSelectedContour(c.guid) :
                                this.props.addSelectedContour(c.guid)
                            }
                        >
                            {c.tag}
                            <ListItemSecondaryAction>
                                <ListItemIcon>
                                    {this.props.selectedContourGuids.find(sc => sc === c.guid) !== undefined ?
                                        <RemoveIcon /> :
                                        <AddIcon />
                                    }
                                </ListItemIcon>
                            </ListItemSecondaryAction>
                        </ListItem>)}
                </List>
            </>
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            contours: state.contours,
            selectedContourGuids: state.selectedContourGuids
        };
    },
    (dispatch: Dispatch<any>) => ({
        addSelectedContour: (guid: string) => {
            dispatch(addSelectedContour(guid));
        },
        removeSelectedContour: (guid: string) => {
            dispatch(removeSelectedContour(guid));
        }
    })
)(ContourListView);
