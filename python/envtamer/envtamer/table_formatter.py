from prettytable import PrettyTable

from envtamer_db.env_variable import EnvVariable

def print_env_table(env_vars: dict[EnvVariable]):
    table = PrettyTable()
    table.field_names = ["Directory", "Key", "Value"]
    for env_var in env_vars:
        table.add_row([env_var.Directory, env_var.Key, env_var.Value])
    print(table)