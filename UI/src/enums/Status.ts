enum STATUS {
  Error = 1,
  Success = -1
}

type Status = STATUS.Success | STATUS.Error;

export { STATUS };
export type { Status };