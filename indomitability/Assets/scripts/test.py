primes = [num for num in range(2, 1001) if all(num % i != 0 for i in range(2, int(num ** 0.5) + 1))]

print(primes)