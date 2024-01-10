import numpy as np
import matplotlib.pyplot as plt

def random_function(x):
    # Define your random function here
    return np.sin(x) + np.random.normal(0, 0.1, size=len(x))

def simulate_fclt(num_functions, sample_size):
    # Generate independent random functions and sum them up
    functions = [random_function(np.linspace(0, 1, sample_size)) for _ in range(num_functions)]
    sum_function = np.sum(functions, axis=0)

    # Plot the individual functions and their sum
    plt.figure(figsize=(10, 6))
    for i in range(num_functions):
        plt.plot(np.linspace(0, 1, sample_size), functions[i], alpha=0.5, label=f'Function {i+1}')
    plt.plot(np.linspace(0, 1, sample_size), sum_function, color='black', linewidth=2, label='Sum of Functions')
    
    plt.title('Functional Central Limit Theorem Simulation')
    plt.xlabel('X')
    plt.ylabel('Y')
    plt.legend()
    plt.show()

# Set the number of functions and sample size
num_functions = 10
sample_size = 1000

# Simulate FCLT
simulate_fclt(num_functions, sample_size)
