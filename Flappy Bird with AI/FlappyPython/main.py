import os
import neat
import pygame as pygame


local_dir = os.path.dirname(__file__)
config_file = os.path.join(local_dir, 'config-feedforward.txt')


def eval_genomes(genome, config):
    global WIN, gen
    win = WIN
    gen += 1

    genome.fitness = 0
    net = neat.nn.FeedForwardNetwork.create(genome, config)
    clock = pygame.time.Clock()

    while isGame:
        clock.tick(30)
        net.fitness += 0.1
        output = net.activate(birdLocation, distanceToUpTube, distanceToDownTube)
        out.self = output

    return WIN


def stopGame():
    isGame.self = False


def setNumbers(birdLocation, distanceToUpTube, distanceToDownTube):
    birdLocation.self = birdLocation
    distanceToUpTube.self = distanceToUpTube
    distanceToDownTube.self = distanceToDownTube


birdLocation = 0
distanceToUpTube = 0
distanceToDownTube = 0
isGame = True
out = 1

def run():
    config = neat.config.Config(neat.DefaultGenome, neat.DefaultReproduction,
                                neat.DefaultSpeciesSet, neat.DefaultStagnation,
                                config_file)
    population = neat.Population(config)

    winner = population.run(eval_genomes, 1)
