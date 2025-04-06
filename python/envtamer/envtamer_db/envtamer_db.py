import os
from envtamer_db.env_variable import EnvVariable, Base
from sqlalchemy import create_engine, select, distinct
from sqlalchemy_utils import database_exists, create_database
from sqlalchemy.orm import Session

class EnvTamerDb:

    def __init__(self):
        self.engine = None

        # get user home
        self.user_folder = os.path.expanduser('~')
        self.db_path = os.path.join(self.user_folder, ".envtamer")
        if not os.path.exists(self.db_path):
            os.makedirs(self.db_path)
        self.db_file = os.path.join(self.db_path, "envtamer.db")
        if os.name == 'nt':  # windows... duh
            self.db_file = self.db_file.replace("\\", "\\\\")

    def ensure_db(self):
        self.engine = create_engine(f"sqlite:///{self.db_file}")

    def create_env_database(self):
        try:
            if os.path.exists(self.db_file):
                print('🛑 Database file already exists. Initialization skipped.')
                self.engine = create_engine(f"sqlite:///{self.db_file}")
            else:
                engine = create_engine(f"sqlite:///{self.db_file}")
                if not database_exists(engine.url):
                    create_database(engine.url)
                    Base.metadata.create_all(engine)
                self.engine = engine

                print('️🗄️Empty database file created successfully.')
            print('🚀 Ready to push and pull env files.')
        except Exception:
            print('🛑 Init went wrong')
            raise

    def save_env_values(self, directory: str, variables: dict):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                for key in variables:
                    stmt = select(EnvVariable).where((EnvVariable.Directory == directory) & (EnvVariable.Key == key))
                    existing_env = env_session.scalars(stmt).one_or_none()
                    if existing_env is None:
                        env_var = EnvVariable(
                            Directory=directory,
                            Key=key,
                            Value=variables[key]
                        )
                        env_session.add(env_var)
                    else:
                        existing_env.value = variables[key]
                env_session.commit()
                print (f'{len(variables)} saved successfully')
        except Exception:
            print('🛑 save envs went wrong')
            raise

    def save_env_value(self, directory: str, key: str, value: str):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                stmt = select(EnvVariable).where(EnvVariable.Directory == directory and EnvVariable.Key == key)
                existing_env = env_session.scalars(stmt).one()
                if existing_env is None:
                    env_var = EnvVariable(
                        Directory=directory,
                        Key=key,
                        Value=value
                    )
                    env_session.add(env_var)
                else:
                    existing_env.value = value
                env_session.commit()
        except Exception:
            print('🛑 save env went wrong:')
            raise

    def get_env_value(self, directory: str, key: str):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                stmt = select(EnvVariable).where(EnvVariable.Directory == directory and EnvVariable.Key == key)
                return env_session.scalars(stmt).one()
        except Exception:
            print('🛑 get env went wrong:')
            raise

    def get_env_values(self, directory: str):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                stmt = select(EnvVariable).where(EnvVariable.Directory == directory)
                return env_session.scalars(stmt).all()
        except Exception:
            print('🛑 get all envs went wrong:')
            raise

    def get_all_directories(self):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                stmt = select(distinct(EnvVariable.Directory))
                return env_session.scalars(stmt).all()
        except Exception:
            print('🛑 get all directories went wrong:')
            raise

    def delete_value(self, directory: str, key: str):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                stmt = select(EnvVariable).where(EnvVariable.Directory == directory and EnvVariable.Key == key)
                env_var = env_session.scalars(stmt).one()
                if env_var is not None:
                    env_session.delete(env_var)
                    env_session.commit()
        except Exception:
            print('🛑 delete env went wrong:')
            raise

    def delete_values(self, directory: str):
        try:
            if self.engine is None:
                self.ensure_db()
            with Session(self.engine) as env_session:
                stmt = select(EnvVariable).where(EnvVariable.Directory == directory)
                env_vars = env_session.scalars(stmt).all()
                for env_var in env_vars:
                    env_session.delete(env_var)
                env_session.commit()
        except Exception:
            print('🛑 delete env went wrong:')
            raise