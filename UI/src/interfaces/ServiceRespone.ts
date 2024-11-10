import { STATUS } from "../enums/Status";

export type ServiceResponse = 
{
    status: STATUS.Success
    data: any
} & 
{
    status: STATUS.Error
    data: undefined
}