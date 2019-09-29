import { BaseModel } from './base-model';

export class UserBaseModel extends BaseModel {
  email: string;
}

export class UserDTO extends UserBaseModel {
  password: string;
}

export class UserViewModel extends BaseModel {
  username: string;
}

export class UserAuthModel extends UserViewModel {
  token: string;
}
